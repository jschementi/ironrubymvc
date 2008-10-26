class TaggingsController < ApplicationController
  def create
    person = Person.find(params[:person_id])
    # person.taggings.create(:tag => Tag.find_or_create_by_name(params[:name]), :taggable => Picture.find(params[:picture_id]))
    #FIXME: this is not the way you'd do this with ActiveRecord, but we need to do some work for setting values that are
    # AM models correctly
    person.taggings.create(:tag_id => Tag.find_or_create_by_name(params[:name]).id, :taggable_id => Picture.find(params[:picture_id]).id, :taggable_type => "Picture")
    
    flash[:notice] = "Tagged it!"
    redirect_to :back
  end
end
