class MyControllerFilter < ActionFilter
  
  def on_action_executing(context)
    context.http_context.response.write 'My controller filter<br />'
  end
  
  def on_action_executed(context)
    #noop
  end
  
end