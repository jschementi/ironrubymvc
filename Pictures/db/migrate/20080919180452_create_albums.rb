class CreateAlbums < ActiveRecord::Migration
  def self.up
    create_table :albums do |t|
      t.integer :person_id
      t.string :name

      t.timestamps
    end
  end

  def self.down
    drop_table :albums
  end
end
