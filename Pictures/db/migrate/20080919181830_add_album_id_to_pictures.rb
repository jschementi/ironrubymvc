class AddAlbumIdToPictures < ActiveRecord::Migration
  def self.up
    add_column :pictures, :album_id, :integer
  end

  def self.down
    remove_column :pictures, :album_id
  end
end
