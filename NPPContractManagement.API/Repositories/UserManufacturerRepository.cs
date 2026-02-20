using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NPPContractManagement.API.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NPPContractManagement.API.Repositories
{
    public class UserManufacturerRepository : IUserManufacturerRepository
    {
        private readonly ApplicationDbContext _context;

        public UserManufacturerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<int>> GetManufacturerIdsForUserAsync(int userId)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await _context.Database.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ManufacturerId FROM UserManufacturers WHERE UserId = @UserId";
            var p = cmd.CreateParameter(); p.ParameterName = "@UserId"; p.Value = userId; cmd.Parameters.Add(p);
            var result = new List<int>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) result.Add(reader.GetInt32(0));
            return result;
        }

        public async Task<IReadOnlyList<int>> GetUserIdsForManufacturerAsync(int manufacturerId)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await _context.Database.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserId FROM UserManufacturers WHERE ManufacturerId = @ManufacturerId";
            var p = cmd.CreateParameter(); p.ParameterName = "@ManufacturerId"; p.Value = manufacturerId; cmd.Parameters.Add(p);
            var result = new List<int>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) result.Add(reader.GetInt32(0));
            return result;
        }

        public async Task SyncUserManufacturersAsync(int userId, IEnumerable<int> manufacturerIds, string assignedBy)
        {
            var targetIds = new HashSet<int>(manufacturerIds ?? Enumerable.Empty<int>());

            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                await _context.Database.OpenConnectionAsync();

            await using var dbTx = await _context.Database.BeginTransactionAsync();
            await SyncInternalAsync(userId, targetIds, assignedBy, dbTx);
            await dbTx.CommitAsync();
        }

        private async Task SyncInternalAsync(int userId, HashSet<int> targetIds, string assignedBy, IDbContextTransaction dbTx)
        {
            var conn = _context.Database.GetDbConnection();
            var tx = dbTx.GetDbTransaction();

            // Load existing manufacturer ids
            var existingIds = new HashSet<int>();
            await using (var loadCmd = conn.CreateCommand())
            {
                loadCmd.Transaction = tx;
                loadCmd.CommandText = "SELECT ManufacturerId FROM UserManufacturers WHERE UserId = @UserId";
                var p = loadCmd.CreateParameter(); p.ParameterName = "@UserId"; p.Value = userId; loadCmd.Parameters.Add(p);
                await using var r = await loadCmd.ExecuteReaderAsync();
                while (await r.ReadAsync()) existingIds.Add(r.GetInt32(0));
            }

            // Determine deletions
            var toDelete = existingIds.Where(id => !targetIds.Contains(id)).ToList();
            if (toDelete.Count > 0)
            {
                var inParams = string.Join(",", toDelete.Select((_, i) => $"@m{i}"));
                await using var delCmd = conn.CreateCommand();
                delCmd.Transaction = tx;
                delCmd.CommandText = $"DELETE FROM UserManufacturers WHERE UserId = @UserId AND ManufacturerId IN ({inParams})";
                var pUser = delCmd.CreateParameter(); pUser.ParameterName = "@UserId"; pUser.Value = userId; delCmd.Parameters.Add(pUser);
                for (int i = 0; i < toDelete.Count; i++) { var p = delCmd.CreateParameter(); p.ParameterName = $"@m{i}"; p.Value = toDelete[i]; delCmd.Parameters.Add(p); }
                await delCmd.ExecuteNonQueryAsync();
            }

            // Determine insertions
            var toInsert = targetIds.Where(id => !existingIds.Contains(id)).ToList();
            if (toInsert.Count > 0)
            {
                foreach (var mId in toInsert)
                {
                    await using var insCmd = conn.CreateCommand();
                    insCmd.Transaction = tx;
                    insCmd.CommandText = "INSERT INTO UserManufacturers (UserId, ManufacturerId, AssignedDate, AssignedBy) VALUES (@UserId, @ManufacturerId, @AssignedDate, @AssignedBy)";
                    var p1 = insCmd.CreateParameter(); p1.ParameterName = "@UserId"; p1.Value = userId; insCmd.Parameters.Add(p1);
                    var p2 = insCmd.CreateParameter(); p2.ParameterName = "@ManufacturerId"; p2.Value = mId; insCmd.Parameters.Add(p2);
                    var p3 = insCmd.CreateParameter(); p3.ParameterName = "@AssignedDate"; p3.Value = System.DateTime.UtcNow; insCmd.Parameters.Add(p3);
                    var p4 = insCmd.CreateParameter(); p4.ParameterName = "@AssignedBy"; p4.Value = assignedBy ?? string.Empty; insCmd.Parameters.Add(p4);
                    await insCmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

