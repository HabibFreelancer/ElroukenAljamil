using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations
{
    public class DepositWorkflowConfiguration : IEntityTypeConfiguration<DepositWorkflow>
    {
        public void Configure(EntityTypeBuilder<DepositWorkflow> builder)
        {
            builder.ToTable("DepositWorkflows");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(1000);

            builder.HasOne(e => e.Category)
                   .WithMany()
                   .HasForeignKey(e => e.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Steps)
                   .WithOne(s => s.Workflow)
                   .HasForeignKey(s => s.WorkflowId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.CategoryId);
            builder.HasIndex(e => e.IsActive);
        }
    }

    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.ToTable("WorkflowSteps");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Subtitle).HasMaxLength(500);
            builder.Property(e => e.StepKey).IsRequired().HasMaxLength(100);

            builder.HasMany(e => e.Fields)
                   .WithOne(f => f.Step)
                   .HasForeignKey(f => f.StepId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.WorkflowId, e.StepOrder });
        }
    }

    public class StepFieldConfiguration : IEntityTypeConfiguration<StepField>
    {
        public void Configure(EntityTypeBuilder<StepField> builder)
        {
            builder.ToTable("StepFields");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FieldKey).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Label).IsRequired().HasMaxLength(200);
            builder.Property(e => e.FieldType).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Placeholder).HasMaxLength(500);
            builder.Property(e => e.Options).HasColumnType("nvarchar(max)");
            builder.Property(e => e.DefaultValue).HasMaxLength(500);
            builder.Property(e => e.Suffix).HasMaxLength(50);
            builder.Property(e => e.HelperText).HasMaxLength(500);
            builder.Property(e => e.ValidationRegex).HasMaxLength(500);
            builder.Property(e => e.VisibilityCondition).HasMaxLength(1000);

            builder.HasIndex(e => new { e.StepId, e.DisplayOrder });
        }
    }
}
