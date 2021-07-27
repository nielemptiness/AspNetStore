using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Contracts.Interfaces;
using Store.Core.Contracts.Models;

namespace Store.Core.Services.Records.Queries.UpdateRecord
{
    public class MarkAsSoldCommand : IRequest, IIdentity
    {
        public Guid Id { get; set; }
    }
    
    public class RecordMarkAsSoldCommandHandler : IRequestHandler<MarkAsSoldCommand>
    {
        private readonly IRecordService _recordService;
        private readonly ICacheService _cacheService;

        public RecordMarkAsSoldCommandHandler(IRecordService recordService, ICacheService cacheService)
        {
            _recordService = recordService;
            _cacheService = cacheService;
        }
        public async Task<Unit> Handle(MarkAsSoldCommand request, CancellationToken cancellationToken)
        {
            var cacheRecord = await _cacheService.GetCacheAsync<Record>(request.Id.ToString(), cancellationToken);
            
            var record = cacheRecord ?? await _recordService.GetRecord(request.Id);

            if (record == null)
                throw new Exception($"Record {request.Id} is not found!");
            
            if (record.IsSold)
                throw new Exception("This record already has been sold!");

            await _recordService.MarkRecordAsSold(record.Id);
            
            await _cacheService.AddCacheAsync(RecordCacheHelper.MarkRecordAsSold(record), 
                TimeSpan.FromMinutes(5), cancellationToken);
            
            return Unit.Value;
        }
    }
}