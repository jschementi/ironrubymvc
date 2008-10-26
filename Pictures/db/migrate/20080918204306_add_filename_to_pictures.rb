class AddFilenameToPictures < ActiveRecord::Migration
  def self.up
    add_column :pictures, :filename, :string
  end

  def self.down
    remove_column :pictures, :filename
  end
end
