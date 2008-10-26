class RenameFriendships < ActiveRecord::Migration
  def self.up
    rename_table :friendships, :follows
  end

  def self.down
    rename_table :follows, :friendships
  end
end
