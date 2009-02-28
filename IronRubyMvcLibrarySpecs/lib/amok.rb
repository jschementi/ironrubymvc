# Amok -- a compact mock library

# Copyright (C) 2008 Christian Neukirchen <purl.org/net/chneukirchen>
#
# Amok is freely distributable under the terms of an MIT-style license.
# See COPYING or http://www.opensource.org/licenses/mit-license.php.

class Amok
  VERSION = '0.1'
  
  attr_reader :obj
  
  class Failed < RuntimeError
    attr_accessor :errors
  end
  
  def self.with(obj)
    mock = new(obj)
    yield obj, mock
    mock.validate
  end
  
  def self.make(hash, &block)
    a = new(Object.new, &block)
    hash.each { |key, value| a.on(key) { value } }
    a.obj
  end
  
  @@uuid = 0
  def uuid
    @@uuid += 1
  end
  
  def initialize(obj, &block)
    @obj = obj
    @called = {}
    @previous = {}
    instance_eval(&block)  if block
  end
  
  def on(method=nil, args=nil, n=nil, &block)
    return NiceProxy.new(self, n)  unless method || block
    
    called = @called
    id = [method, args]
    called[id] = n
    _previous = @previous
    
    mock = self
    (class << @obj; self; end).class_eval {
      if block
        current = "__current_#{method}_#{mock.uuid}__amok__"
        define_method(current, &block)
      end
      begin
        previous = "__previous_#{method}_#{mock.uuid}__amok__"
        alias_method previous, method
      rescue NameError
        previous = nil
      end
      _previous[method] ||= previous
      define_method(method) { |*actual_args|
        if args.nil? || args == actual_args
          case called[id]
          when Numeric;  called[id] -= 1
          when false;    called[id] = true
          end
          __send__(current || previous, *actual_args)
        else
          __send__(previous, *actual_args)
        end
      }
    }
  end
  
  def previous(method, *args, &block)
    @obj.__send__(@previous[method], *args, &block)
  end
  
  def need(method=nil, args=nil, n=false, &block)
    unless block
      case method
      when nil;        NiceProxy.new(self, n)        # mock.need.foo
      when Numeric;    NiceProxy.new(self, method)   # mock.need(3).foo
      end
    else
      on(method, args, n, &block)
    end
  end
  
  def never(method=nil, args=nil)
    return NiceProxy.new(self, 0)  if !method
    on(method, args, 0) {
      # should we raise here?
    }
  end
  
  def errors
    @called.reject { |k, v|
      v == 0 ||                 # run the right number of times
      v == true ||              # run at all
      v == nil                  # run? who cares?
    }.map { |(m, a), v|
      msg = m.to_s
      msg << "(#{a.map { |x| x.inspect }.join(", ")})"  if a
      if v == false
        msg << " was not called."
      else
        msg << " was called #{v.abs} times #{v < 0 ? "too often" : "too few"}."
      end
    }
  end
  
  def successful?
    errors.empty?
  end
  
  def validate
    unless successful?
      ex = Failed.new(errors.join("  "))
      ex.errors = errors.dup
      raise ex
    end
  end
  
  def cleanup!
    _previous = @previous
    (class << @obj; self; end).class_eval {
      _previous.each { |old, new|
        new ? alias_method(old, new) : undef_method(old)
      }
      methods.each { |m| undef_method  if m =~ /__amok__\Z/ }
    }
    @obj
  end
  
  class NiceProxy
    instance_methods.each { |name|
      undef_method name  unless name =~ /^__|^instance_eval$/
    }
    
    def initialize(obj, n=nil)
      @obj, @n = obj, n
    end
    
    def method_missing(name, *args, &block)
      args = nil  if args.empty?   # allow any arguments when none are mentioned
      @obj.on(name, args, @n, &block)
      self
    end
  end
end
