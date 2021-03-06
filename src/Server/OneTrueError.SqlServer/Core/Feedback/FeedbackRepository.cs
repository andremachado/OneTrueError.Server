﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.App.Core.Feedback;

namespace OneTrueError.SqlServer.Core.Feedback
{
    [Component]
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public FeedbackRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FeedbackEntity> FindPendingAsync(string reportId)
        {
            return await _unitOfWork.FirstOrDefaultAsync<FeedbackEntity>(new {ErrorId = reportId});
        }

        public async Task UpdateAsync(FeedbackEntity feedback)
        {
            await _unitOfWork.UpdateAsync(feedback);
        }

        public async Task<IReadOnlyList<string>> GetEmailAddressesAsync(int incidentId)
        {
            var emailAddresses = new List<string>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "SELECT distinct EmailAddress FROM IncidentFeedback WHERE IncidentId = @id AND EmailAddress IS NOT NULL";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        emailAddresses.Add(reader.GetString(0));
                    }
                }
            }

            return emailAddresses;
        }
    }
}