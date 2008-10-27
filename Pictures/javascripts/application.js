switchPictureResolution = function(size) {
  var picture = $('picture');
  switch(size) {
  case 'small':
    picture.addClassName('picture_small');
    picture.removeClassName('picture_medium');
    picture.removeClassName('picture_full');
    break;
  case 'medium':
    picture.addClassName('picture_medium');
    picture.removeClassName('picture_small');
    picture.removeClassName('picture_full');
    break;
  case 'full':
    picture.addClassName('picture_full');
    picture.removeClassName('picture_medium');
    picture.removeClassName('picture_small');
    break;
  }
}

onImageMouseOver = function(anchor) {
    popup = getPopup(anchor)
    if(popup.show)
        popup.show();
    else
        popup.style.display = "inline"
}
onImageMouseOut = function(anchor) {
    popup = getPopup(anchor)
    if (popup.hide)
        popup.hide();
    else
        popup.style.display = "none"
}
onPopupMouseOver = function(popupDiv) {
    if (popupDiv.show)
        popupDiv.show();
    else
        popupDiv.style.display = "inline"
}
onPopupMouseOut = function(popupDiv) {
    if(popupDiv.hide)
        popupDiv.hide();
    else   
        popupDiv.style.display = "none"
}
getPopup = function(anchor) {
    if (anchor.parentNode.childElements) {
        return anchor.parentNode.childElements().last()
    } else {
        return anchor.parentNode.children[anchor.parentNode.children.length - 1]
    }
}