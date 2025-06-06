﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TalkVN.Domain.Entities.SystemEntities.Group;


namespace TalkVN.DataAccess.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> modelBuilder)
        {
            //primary key
            modelBuilder.HasKey(g => g.Id);

            // Configure relationship with User
            modelBuilder
                .HasOne(g => g.Creator)
                .WithMany(u => u.Groups)
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with TextChat
            modelBuilder
                .HasMany(g => g.TextChats)
                .WithOne(c => c.Group)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with UserGroup
            modelBuilder
                .HasMany(g => g.UserGroups)
                .WithOne(ugr => ugr.Group)
                .HasForeignKey(ugr => ugr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with GroupInvitation
            modelBuilder
                .HasMany(g => g.GroupInvitations)
                .WithOne(gi => gi.Group)
                .HasForeignKey(gi => gi.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with JoinGroupRequest
            modelBuilder
                .HasMany(g => g.JoinGroupRequests)
                .WithOne(jgr => jgr.Group)
                .HasForeignKey(jgr => jgr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure relationship with GroupNotification
            modelBuilder
                .HasMany(g => g.GroupNotifications)
                .WithOne(gn => gn.Group)
                .HasForeignKey(gn => gn.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure relationship with MeetingSchedule
            modelBuilder
                .HasMany(g => g.MeetingSchedules)
                .WithOne(ms => ms.Group)
                .HasForeignKey(ms => ms.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
