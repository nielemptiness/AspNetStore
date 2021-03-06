using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Store.Core.Contracts.Interfaces.Services;
using Store.Core.Services.Internal.Records.Commands.DeleteRecord;
using Xunit;
using Record = Store.Core.Contracts.Domain.Record;

namespace Store.Services.Records.Tests.Handlers
{
    public class DeleteRecordHandlerTests
    {
        private readonly Mock<IRecordService> _recordService;
        private readonly Mock<ICacheService> _cacheService;

        public DeleteRecordHandlerTests()
        {
            _recordService = new Mock<IRecordService>();
            _cacheService = new Mock<ICacheService>();
        }

        [Fact]
        public async Task HandleDeleteRequests_DeletesRecord_FromDbAndCache_WhenRequestCorrect_AndFoundInDb()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new DeleteRecordCommand
            {
                Id = id
            };

            var expectedRecord = new Record
            {
                Id = id
            };
            
            _cacheService.Setup(arg => arg.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Record) null);
            _recordService.Setup(x => x.GetRecordAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecord);
            _recordService.Setup(x => x.DeleteRecordAsync(id, It.IsAny<CancellationToken>()));
            _cacheService.Setup(x => x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None));

            var handle = new DeleteRecordCommandHandler(_recordService.Object, _cacheService.Object);

            await handle.Handle(request, CancellationToken.None);
            
            //Assert
            _cacheService.Verify(x=>x.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()), Times.Once);
            _recordService.Verify(x => x.GetRecordAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _recordService.Verify(x=>x.DeleteRecordAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _cacheService.Verify(x=>x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task HandleDeleteRequests_DeletesRecord_FromDbAndCache_WhenRequestCorrect_AndFoundInCache()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new DeleteRecordCommand
            {
                Id = id
            };

            var expectedRecord = new Record
            {
                Id = id
            };
            
            _cacheService.Setup(arg => arg.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecord);
            _recordService.Setup(x => x.DeleteRecordAsync(id, CancellationToken.None));
            _cacheService.Setup(x => x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None));

            var handle = new DeleteRecordCommandHandler(_recordService.Object, _cacheService.Object);

            await handle.Handle(request, CancellationToken.None);
            
            //Assert
            _cacheService.Verify(x=>x.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()), Times.Once);
            _recordService.Verify(x => x.GetRecordAsync(id, CancellationToken.None), Times.Never);
            _recordService.Verify(x=>x.DeleteRecordAsync(id, CancellationToken.None), Times.Once);
            _cacheService.Verify(x=>x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public void HandleDeleteRequests_Throws_WhenNoRecord()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new DeleteRecordCommand
            {
                Id = id
            };
            
            _cacheService.Setup(arg => arg.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Record) null);
            _recordService.Setup(x => x.GetRecordAsync(id, CancellationToken.None))
                .ReturnsAsync((Record) null);
            _recordService.Setup(x => x.DeleteRecordAsync(id, CancellationToken.None));
            _cacheService.Setup(x => x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None));
            
            var handle = new DeleteRecordCommandHandler(_recordService.Object, _cacheService.Object);
            
            Func<Task> handleRequest = async () => await handle.Handle(request, CancellationToken.None);
            handleRequest.Should().Throw<ArgumentException>();
            
            _cacheService.Verify(x=>x.GetCacheAsync<Record>(id.ToString(), It.IsAny<CancellationToken>()), Times.Once);
            _recordService.Verify(x => x.GetRecordAsync(id, CancellationToken.None), Times.Once);
            _recordService.Verify(x=>x.DeleteRecordAsync(id, CancellationToken.None), Times.Never);
            _cacheService.Verify(x=>x.DeleteCacheAsync<Record>(id.ToString(), CancellationToken.None), Times.Never);
        }
    }
}