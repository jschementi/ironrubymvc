class AddEmailToPeople < ActiveRecord::Migration
  def self.up
    add_column :people, :email, :string
  end

  def self.down
    remove_column :people, :email
  end
end
