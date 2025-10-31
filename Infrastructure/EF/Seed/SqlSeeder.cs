using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.EF.Seed
{
    public static class SqlSeeder
    {
        public static async Task SeedAsync(EFContext ctx)
        {
            if (ctx.Teams.Any()) return;

            var teamA = new Team { Name = "Team A" };
            teamA.Agents.Add(new Agent { Name = "Lead A", Seniority = SeniorityLevel.Lead, Shift = ShiftType.Morning });
            teamA.Agents.Add(new Agent { Name = "Mid A1", Seniority = SeniorityLevel.Mid, Shift = ShiftType.Morning });
            teamA.Agents.Add(new Agent { Name = "Mid A2", Seniority = SeniorityLevel.Mid, Shift = ShiftType.Morning });
            teamA.Agents.Add(new Agent { Name = "Junior A", Seniority = SeniorityLevel.Junior, Shift = ShiftType.Morning });

            var teamB = new Team { Name = "Team B" };
            teamB.Agents.Add(new Agent { Name = "Senior B", Seniority = SeniorityLevel.Senior, Shift = ShiftType.Evening });
            teamB.Agents.Add(new Agent { Name = "Mid B", Seniority = SeniorityLevel.Mid, Shift = ShiftType.Evening });
            teamB.Agents.Add(new Agent { Name = "Junior B1", Seniority = SeniorityLevel.Junior, Shift = ShiftType.Evening });
            teamB.Agents.Add(new Agent { Name = "Junior B2", Seniority = SeniorityLevel.Junior, Shift = ShiftType.Evening });

            var teamC = new Team { Name = "Team C" };
            teamC.Agents.Add(new Agent { Name = "Mid C1", Seniority = SeniorityLevel.Mid, Shift = ShiftType.Night });
            teamC.Agents.Add(new Agent { Name = "Mid C2", Seniority = SeniorityLevel.Mid, Shift = ShiftType.Night });

            var overflowTeam = new Team { Name = "Overflow Team" };
            for (int i = 1; i <= 6; i++)
            {
                overflowTeam.Agents.Add(new Agent
                {
                    Name = $"Overflow Jr {i}",
                    Seniority = SeniorityLevel.Junior,
                    Shift = ShiftType.Morning
                });
            }

            ctx.Teams.AddRange(teamA, teamB, teamC, overflowTeam);
            await ctx.SaveChangesAsync();
        }
    }
}
