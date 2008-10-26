class ChangePictureDataToBinary < ActiveRecord::Migration
  def self.up
    change_column :pictures, :data, :binary
  end

  def self.down
    change_column :pictures, :data, :text    
  end
end
