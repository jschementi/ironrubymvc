class AlbumsController < ApplicationController

  def index
    @person = person
    @albums = albums
    return_view "index", "layout"
  end
  
  def new
    @album = Album.new
    return_view "new", "layout"
  end
  
  def show
    @album = Album.find(params[:id])
    return_view "show", "layout"
  end
  
  def create
    @album = current_person.albums.build(params[:album])
    if @album.save
      flash[:notice] = "Your album has been created!"
      redirect_to @album
    else
      flash[:error] = "There was a problem adding your album."
      render :action => 'new'
    end
  end
  
  private
    def person
      @person ||= (Person.first(:conditions => {:id => params[:person_id]}) || current_person)
    end
    
    def albums
      person.blank? ? Album.all : person.albums
    end
end
