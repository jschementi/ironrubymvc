class SessionsController < ApplicationController
  def create
    person = Person.authenticate(params[:email], params[:password])
    if person
      session[:person_id] = person.id
      flash[:notice] = "Welcome back, #{person.name}!"
      redirect_to person_pictures_url(person.id)
    else
      flash[:error] = "There was a problem with your email, password, or both.  Please double-check them and try again."
      render :action => 'new'
    end
  end
  
  def destroy
    reset_session
    flash[:notice] = "Come again soon!"
    redirect_to root_url
  end
end
