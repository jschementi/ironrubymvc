$: << File.dirname(__FILE__) + "/../Models"
$: << File.dirname(__FILE__) + "/../lib/activesupport/lib"
$: << File.dirname(__FILE__) + "/../lib/activerecord/lib"
$: << File.dirname(__FILE__) + "/../lib/activemodel/lib"
$: << File.dirname(__FILE__) + "/sinatra/lib"
$: << File.dirname(__FILE__) + "/rack/lib"
require 'rubygems'
require 'sinatra'
require 'active_model'
require 'in_memory_model'
require 'picture'

get '/pictures/:id/serve*' do
  @picture = Picture.find(params[:id])
  send_data(
    params['splat'] == '_thumbnail' ? @picture.thumbnail_data || @picture.data : @picture.data,
    :filename => @picture.filename, 
    :mime_type => @picture.mime_type
  )
end

