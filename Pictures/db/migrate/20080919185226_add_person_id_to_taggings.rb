class AddPersonIdToTaggings < ActiveRecord::Migration
  def self.up
    add_column :taggings, :person_id, :integer
  end

  def self.down
    remove_column :taggings, :person_id
  end
end
