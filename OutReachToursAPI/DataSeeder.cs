using OutReachToursAPI.Data;
using OutReachToursAPI.Models;

namespace OutReachToursAPI
{
    public static class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(new CustomRole
                {
                    Id = "admin_role_id",
                    Name = "Admin",
                    Permissions = new List<string> { 
                        "view_overview", "view_tours", "create_tours", "edit_tours", "delete_tours",
                        "view_crm", "view_all_leads", "manage_crm_stages", "reassign_leads",
                        "view_pos", "create_pos_transaction", "view_global_ledger",
                        "view_team", "manage_users", "manage_roles" 
                    }
                });
            }

            if (!context.Stages.Any())
            {
                context.Stages.AddRange(
                    new PipelineStage { Id = "stage_1", Name = "Lead", Color = "#3b82f6", Order = 1 },
                    new PipelineStage { Id = "stage_2", Name = "Contacted", Color = "#f59e0b", Order = 2 },
                    new PipelineStage { Id = "stage_3", Name = "Qualified", Color = "#8b5cf6", Order = 3 },
                    new PipelineStage { Id = "stage_4", Name = "Proposal Sent", Color = "#ec4899", Order = 4 },
                    new PipelineStage { Id = "stage_5", Name = "Won", Color = "#10b981", Order = 5 }
                );
            }

            context.SaveChanges();
        }
    }
}
