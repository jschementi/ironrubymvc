class CommentsController < ApplicationController
  before_filter :authenticate
  
  def new
    @picture = Picture.find(params[:picture_id])
    @comment = @picture.comments.build
  end
  
  def index
    @comments = Picture.find(params[:picture_id]).comments
    render :xml => @comments
  end
  
  def create
    @comment = current_person.comments.build(params[:comment])
    if @comment.save
      flash[:notice] = "Your voice has been heard!"
      redirect_to :back
    else
      flash[:error] = "There was a problem submitting your comment"
      render :action => 'new'
    end
  end
end
