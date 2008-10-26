class CreatePictures < ActiveRecord::Migration
  def self.up
    create_table :pictures do |t|
      t.string :title
      t.integer :person_id
      t.string :mime_type

      t.timestamps
    end
  end

  def self.down
    drop_table :pictures
  end
end
