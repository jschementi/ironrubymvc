class MungeFollowsColumns < ActiveRecord::Migration
  def self.up
    rename_column :follows, :friend_id, :following_id
    rename_column :follows, :originator_id, :follower_id
  end

  def self.down
    rename_column :follows, :following_id, :friend_id
    rename_column :follows, :follower_id, :originator_id
  end
end
