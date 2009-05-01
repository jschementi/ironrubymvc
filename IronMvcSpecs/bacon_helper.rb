$: << File.dirname(__FILE__) + '/bin'
require 'bacon'
require 'mscorlib'
require File.dirname(__FILE__) + "/lib/amok.rb"

#load_assembly 'System.Web.Mvc.IronRuby, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
load_assembly 'System.Web.Mvc.IronRuby'
load_assembly 'BugWorkarounds'
