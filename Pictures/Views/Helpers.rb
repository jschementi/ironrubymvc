def say_hi
  "hi from a helper!!!!"
end

def flash
  @flash ||= {}
end

def commentor(comment)
  if comment.person
    comment.person.name
  else
    "Anonymous"
  end
end

def picture_index_title
  if view_data.person
    if view_data.person == current_person
      "Your Pictures"
    else
      "#{view_data.person.name}#{view_data.person.name[-1, 1] == "s" ? "'" : "'s"} Pictures"
    end
  else
    nil
  end
end

def current_person
  @current_person ||= logged_in? ? Person.find(session[:person_id]) : nil
end

def logged_in?
  !session[:person_id].blank?
end

def session
  @session ||= {}
end