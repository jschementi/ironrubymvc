class PeopleController < ApplicationController
  
  def show
    @person = Person.find(params[:id])
    respond_to do |format|
      format.html
      format.js { render :json => @person }
      format.xml { render :xml => @person }
    end    
  end
  
  def index
    @people = Person.search(params[:term])
    respond_to do |format|
      format.html
      format.js { render :json => @people }
      format.xml { render :xml => @people }    
    end
  end
  
  def new
    @person = Person.new
  end
  
  def edit
    @person = current_person
  end
  
  def create
    @person = Person.new(params[:person])
    if @person.save
      flash[:notice] = "Welcome, and thanks for registering!"
      redirect_to new_session_url
    else
      flash[:error] = "There was a problem registering.  Please try again."
      render :action => 'new'
    end
  end
end
