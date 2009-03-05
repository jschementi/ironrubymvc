class MyProductFilter < ActionFilter
  
  def on_action_executing(context)
    context.http_context.response.write 'MyFilter'
  end
  
  def on_action_executed(context)
    #noop
  end
  
end