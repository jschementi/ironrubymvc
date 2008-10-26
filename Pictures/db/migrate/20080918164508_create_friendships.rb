class CreateFriendships < ActiveRecord::Migration
  def self.up
    create_table :friendships do |t|
      t.integer :originator_id
      t.integer :friend_id

      t.timestamps
    end
  end

  def self.down
    drop_table :friendships
  end
end
