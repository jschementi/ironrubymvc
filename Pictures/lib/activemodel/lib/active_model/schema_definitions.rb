require 'date'
require 'set'
require 'bigdecimal'
require 'bigdecimal/util'

module ActiveModel
  module SchemaDefinitions #:nodoc:
    # An abstract definition of a column in a table.
    class Column
      TRUE_VALUES = [true, 1, '1', 't', 'T', 'true', 'TRUE'].to_set

      module Format
        ISO_DATE = /\A(\d{4})-(\d\d)-(\d\d)\z/
        ISO_DATETIME = /\A(\d{4})-(\d\d)-(\d\d) (\d\d):(\d\d):(\d\d)(\.\d+)?\z/
      end

      attr_reader :name, :default, :type, :limit, :null, :native_type, :precision, :scale
      attr_accessor :primary

      # Instantiates a new column in the table.
      #
      # +name+ is the column's name, such as <tt>supplier_id</tt> in <tt>supplier_id int(11)</tt>.
      # +default+ is the type-casted default value, such as +new+ in <tt>sales_stage varchar(20) default 'new'</tt>.
      # +native_type+ is only used to extract the column's length, if necessary. For example +60+ in <tt>company_name varchar(60)</tt>.
      # +null+ determines if this column allows +NULL+ values.
      def initialize(name, default, native_type = nil, null = true)
        @name, @native_type, @null = name, native_type, null
        @limit, @precision, @scale = extract_limit(native_type), extract_precision(native_type), extract_scale(native_type)
        @type = simplified_type(native_type)
        @default = extract_default(default)

        @primary = nil
      end

      def text?
        type == :string || type == :text
      end

      def number?
        type == :integer || type == :float || type == :decimal
      end

      def has_default?
        !default.nil?
      end

      # Returns the Ruby class that corresponds to the abstract data type.
      def klass
        case type
          when :integer       then Fixnum
          when :float         then Float
          when :decimal       then BigDecimal
          when :datetime      then Time
          when :date          then Date
          when :timestamp     then Time
          when :time          then Time
          when :text, :string then String
          when :binary        then String
          when :boolean       then Object
        end
      end

      # Casts value (which is a String) to an appropriate instance.
      def type_cast(value)
        return nil if value.nil?
        case type
          when :string    then value
          when :text      then value
          when :integer   then value.to_i rescue value ? 1 : 0
          when :float     then value.to_f
          when :decimal   then self.class.value_to_decimal(value)
          when :datetime  then self.class.string_to_time(value)
          when :timestamp then self.class.string_to_time(value)
          when :time      then self.class.string_to_dummy_time(value)
          when :date      then self.class.string_to_date(value)
          when :binary    then self.class.binary_to_string(value)
          when :boolean   then self.class.value_to_boolean(value)
          else value
        end
      end

      def type_cast_code(var_name)
        case type
          when :string    then nil
          when :text      then nil
          when :integer   then "(#{var_name}.to_i rescue #{var_name} ? 1 : 0)"
          when :float     then "#{var_name}.to_f"
          when :decimal   then "#{self.class.name}.value_to_decimal(#{var_name})"
          when :datetime  then "#{self.class.name}.string_to_time(#{var_name})"
          when :timestamp then "#{self.class.name}.string_to_time(#{var_name})"
          when :time      then "#{self.class.name}.string_to_dummy_time(#{var_name})"
          when :date      then "#{self.class.name}.string_to_date(#{var_name})"
          when :binary    then "#{self.class.name}.binary_to_string(#{var_name})"
          when :boolean   then "#{self.class.name}.value_to_boolean(#{var_name})"
          else nil
        end
      end

      # Returns the human name of the column name.
      #
      # ===== Examples
      #  Column.new('sales_stage', ...).human_name # => 'Sales stage'
      def human_name
        Base.human_attribute_name(@name)
      end

      def extract_default(default)
        type_cast(default)
      end

      class << self
        # Used to convert from Strings to BLOBs
        def string_to_binary(value)
          value
        end

        # Used to convert from BLOBs to Strings
        def binary_to_string(value)
          value
        end

        def string_to_date(string)
          return string unless string.is_a?(String)
          return nil if string.empty?

          fast_string_to_date(string) || fallback_string_to_date(string)
        end

        def string_to_time(string)
          return string unless string.is_a?(String)
          return nil if string.empty?

          fast_string_to_time(string) || fallback_string_to_time(string)
        end

        def string_to_dummy_time(string)
          return string unless string.is_a?(String)
          return nil if string.empty?

          string_to_time "2000-01-01 #{string}"
        end

        # convert something to a boolean
        def value_to_boolean(value)
          if value.is_a?(String) && value.blank?
            nil
          else
            TRUE_VALUES.include?(value)
          end
        end

        # convert something to a BigDecimal
        def value_to_decimal(value)
          # Using .class is faster than .is_a? and
          # subclasses of BigDecimal will be handled
          # in the else clause
          if value.class == BigDecimal
            value
          elsif value.respond_to?(:to_d)
            value.to_d
          else
            value.to_s.to_d
          end
        end

        protected
          # '0.123456' -> 123456
          # '1.123456' -> 123456
          def microseconds(time)
            ((time[:sec_fraction].to_f % 1) * 1_000_000).to_i
          end

          def new_date(year, mon, mday)
            if year && year != 0
              Date.new(year, mon, mday) rescue nil
            end
          end

          def new_time(year, mon, mday, hour, min, sec, microsec)
            # Treat 0000-00-00 00:00:00 as nil.
            return nil if year.nil? || year == 0

            Time.time_with_datetime_fallback(Base.default_timezone, year, mon, mday, hour, min, sec, microsec) rescue nil
          end

          def fast_string_to_date(string)
            if string =~ Format::ISO_DATE
              new_date $1.to_i, $2.to_i, $3.to_i
            end
          end

          # Doesn't handle time zones.
          def fast_string_to_time(string)
            if string =~ Format::ISO_DATETIME
              microsec = ($7.to_f * 1_000_000).to_i
              new_time $1.to_i, $2.to_i, $3.to_i, $4.to_i, $5.to_i, $6.to_i, microsec
            end
          end

          def fallback_string_to_date(string)
            new_date(*::Date._parse(string, false).values_at(:year, :mon, :mday))
          end

          def fallback_string_to_time(string)
            time_hash = Date._parse(string)
            time_hash[:sec_fraction] = microseconds(time_hash)

            new_time(*time_hash.values_at(:year, :mon, :mday, :hour, :min, :sec, :sec_fraction))
          end
      end

      private
      #FIXME: all this native_type stuff should probably not be here (SQL-specific)
        def extract_limit(native_type)
          $1.to_i if native_type =~ /\((.*)\)/
        end

        def extract_precision(native_type)
          $2.to_i if native_type =~ /^(numeric|decimal|number)\((\d+)(,\d+)?\)/i
        end

        def extract_scale(native_type)
          case native_type
            when /^(numeric|decimal|number)\((\d+)\)/i then 0
            when /^(numeric|decimal|number)\((\d+)(,(\d+))\)/i then $4.to_i
          end
        end

        def simplified_type(field_type)
          case field_type
            when /int/i
              :integer
            when /float|double/i
              :float
            when /decimal|numeric|number/i
              extract_scale(field_type) == 0 ? :integer : :decimal
            when /datetime/i
              :datetime
            when /timestamp/i
              :timestamp
            when /time/i
              :time
            when /date/i
              :date
            when /clob/i, /text/i
              :text
            when /blob/i, /binary/i
              :binary
            when /char/i, /string/i
              :string
            when /boolean/i
              :boolean
          end
        end
    end
  end
end

