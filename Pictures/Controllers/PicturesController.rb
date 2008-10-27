require 'ApplicationController'

class PicturesController < ApplicationController
  
  def index
    @tags = Tag.all
    filtered_pictures(:limit => 13)
    filtered_albums(:limit => 5)
    return_view 'index', 'layout'
  end
  
  def test
    view 'test', 'layout'
  end
  
  def mine
    params[:person_id] = current_person.id
    filtered_pictures(:limit => 13)
    filtered_albums(:limit => 5)
    return_view 'index', 'layout'
  end
  
  def new
    @picture = Picture.new(:album_id => params[:album_id])
  end
  
  def create
    @picture = current_person.pictures.build(params[:picture])
    if @picture.save
      flash[:notice] = "Thanks, we've got it!"
      redirect_to @picture
    else
      
    end
  end
  
  def show
    @picture = Picture.find(params[:id])
    return_view 'show', 'layout'
  end

  def tagged
    @pictures = Picture.tagged_with(params[:tag])
    return_view 'tagged', 'layout'
  end
  
  private
  
  def filtered_pictures(options)
    if person
      @pictures = Picture.find_for(person, options)
    else
      @pictures = Picture.search(params[:term], options)
    end   
  end

  def filtered_albums(options)
    if person
      @albums = Album.find_for(person, options)
    else
      @albums = Album.search(params[:term], options)
    end   
  end
  
  def person
    @person ||= Person.find(params[:person_id]) if params[:person_id]
  end

end