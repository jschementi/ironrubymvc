require 'active_support/core_ext/array/extract_options'
class Array #:nodoc:
    include ActiveSupport::CoreExtensions::Array::ExtractOptions
end

require 'active_support/core_ext/module/attribute_accessors'
require 'active_support/core_ext/module/delegation'

require 'active_model/observing'
# disabled until they're tested
require 'active_model/persistence'
require 'active_model/associations'
require 'active_model/reflection'
require 'active_model/named_scope'
require 'active_model/validations'
require 'active_model/callbacks'
require 'active_model/attribute_methods'
require 'active_model/schema_definitions'
require 'active_model/base'
require 'active_model/errors'