using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Dtos.Enums;
using NetBazaar.Dalc.Interfaces;
using NetBazaar.ViewModels.PostingViewModels;

namespace NetBazaar.Dalc.MongoDb
{
    public class PostingsStoreMongoDb : IPostingsStore
    {
        private readonly IIdentity _currentUser;
        private readonly NetBazaarDatabase _netBazaarDatabase;
        private readonly IMongoDatabase _netBazaarMongoDbDatabase;


        public PostingsStoreMongoDb(IMongoDatabase netBazaarMongoDbDatabase, NetBazaarDatabase netBazaarDatabase,
            IIdentity currentUser)
        {
            _netBazaarMongoDbDatabase = netBazaarMongoDbDatabase;
            _netBazaarDatabase = netBazaarDatabase;
            _currentUser = currentUser;
        }

        public Task DeletePosting(string postingId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteImage(string postingId, ImageReference imageReference)
        {
            throw new NotImplementedException();
        }

        public Task SaveImage(string postingId, ImageReference imageReference)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePosting(PostingViewModel posting)
        {
            throw new NotImplementedException();
        }

        public Task InsertPosting(PostingViewModel postingViewModel)
        {
            var posting = new BsonDocument();

            posting.Add(new BsonElement(nameof(postingViewModel.CategoryId), postingViewModel.CategoryId));
            posting.Add(new BsonElement(nameof(postingViewModel.UserId), postingViewModel.UserId));
            posting.Add(new BsonElement(nameof(postingViewModel.Title), postingViewModel.Title));
            posting.Add(new BsonElement(nameof(postingViewModel.Description), postingViewModel.Description));
            posting.Add(new BsonElement(nameof(postingViewModel.CreationDate), postingViewModel.CreationDate));
            posting.Add(new BsonElement(nameof(postingViewModel.ExpirationDate), postingViewModel.ExpirationDate));
            posting.Add(new BsonElement(nameof(postingViewModel.Type), postingViewModel.Type));

            var fields = new BsonArray();

            foreach (var postingFieldViewModel in postingViewModel.Fields)
            {
                var field = new BsonDocument();

                field.Add(nameof(postingFieldViewModel.BooleanValue), postingFieldViewModel.BooleanValue);
                field.Add(nameof(postingFieldViewModel.DecimalValue), postingFieldViewModel.DecimalValue);
                field.Add(nameof(postingFieldViewModel.IntegerValue), postingFieldViewModel.IntegerValue);
                field.Add(nameof(postingFieldViewModel.TextValue), postingFieldViewModel.TextValue);

                field.Add(new BsonElement(nameof(postingFieldViewModel.CategoryFieldId),
                    postingFieldViewModel.CategoryFieldId));


                fields.Add(field);
            }

            posting.Add("Fields", fields);

            var postingsCollection = _netBazaarMongoDbDatabase.GetCollection<BsonDocument>("Postings");

            postingsCollection.InsertOne(posting);

            postingViewModel.Id = posting["_id"].AsObjectId.ToString();

            return Task.FromResult(0);
        }


        public Task<List<PostingInfoViewModel>> GetPostingsFromCategory(long categoryId, int pageSize,
            int pageNumber)
        {
            var postingsCollection = _netBazaarMongoDbDatabase.GetCollection<BsonDocument>("Postings");
            var query = Builders<BsonDocument>.Filter.Eq(nameof(PostingFieldViewModel.CategoryFieldId), categoryId);
            var postingDocuments = postingsCollection.Find(query)
                .SortBy(post => post[nameof(PostingViewModel.CreationDate)])
                .Skip((pageNumber - 1)*pageSize)
                .Limit(pageSize).ToList();
            var postingInfoViewModels = new List<PostingInfoViewModel>();

            foreach (var postingDocument in postingDocuments)
            {
                var postingInfoViewModel = new PostingInfoViewModel();

                postingInfoViewModel.Id = postingDocument["_id"].AsObjectId.ToString();
                postingInfoViewModel.Title = postingDocument[nameof(PostingViewModel.Title)].AsString;
                postingInfoViewModel.Description = postingDocument[nameof(PostingViewModel.Description)].AsString;

                postingInfoViewModels.Add(postingInfoViewModel);
            }

            return Task.FromResult(postingInfoViewModels);
        }

        public Task<PostingViewModel> GetPosting(string postingId)
        {
            var query = Builders<BsonDocument>.Filter.Eq("id", ObjectId.Parse(postingId));
            var postingsCollection = _netBazaarMongoDbDatabase.GetCollection<BsonDocument>("Postings");
            var postingDocument = postingsCollection.Find(query).FirstOrDefault();

            var postingViewModel = new PostingViewModel();

            postingViewModel.CategoryId = postingDocument[nameof(postingViewModel.CategoryId)].AsInt64;
            postingViewModel.UserId = postingDocument[nameof(postingViewModel.UserId)].AsInt64;
            postingViewModel.Title = postingDocument[nameof(postingViewModel.Title)].AsString;
            postingViewModel.Description = postingDocument[nameof(postingViewModel.Description)].AsString;
            postingViewModel.CreationDate = postingDocument[nameof(postingViewModel.CreationDate)].ToLocalTime();
            postingViewModel.ExpirationDate = postingDocument[nameof(postingViewModel.ExpirationDate)].ToLocalTime();
            postingViewModel.Type = postingDocument[nameof(postingViewModel.Type)].AsInt32;
            postingViewModel.Id = postingDocument["_id"].AsObjectId.ToString();

            var fieldDocuments = postingDocument["Fields"].AsBsonArray;
            var postingCategory =
                _netBazaarDatabase.Categories.Include("CategoryFields")
                    .First(category => category.Id == postingViewModel.CategoryId);

            foreach (var fieldDocument in fieldDocuments)
            {
                var postingFieldViewModel = new PostingFieldViewModel();
                var categoryField =
                    postingCategory.CategoryFields.First(field => field.Id == postingFieldViewModel.CategoryFieldId);

                postingFieldViewModel.Type = categoryField.Type;
                postingFieldViewModel.BooleanValue =
                    fieldDocument[nameof(postingFieldViewModel.BooleanValue)].AsNullableBoolean;
                postingFieldViewModel.DecimalValue =
                    fieldDocument[nameof(postingFieldViewModel.DecimalValue)].AsNullableDouble;
                postingFieldViewModel.IntegerValue =
                    fieldDocument[nameof(postingFieldViewModel.IntegerValue)].AsNullableInt32;
                postingFieldViewModel.TextValue = fieldDocument[nameof(postingFieldViewModel.TextValue)].AsString;
                postingFieldViewModel.CategoryFieldId =
                    fieldDocument[nameof(postingFieldViewModel.CategoryFieldId)].AsInt32;
                postingFieldViewModel.CategoryFieldName =
                    categoryField.Name;

                postingViewModel.Fields.Add(postingFieldViewModel);
            }

            return Task.FromResult(postingViewModel);
        }

        public Task<ImageReference> SaveMainImage(string postingId, ImageReference imageReference)
        {
            throw new NotImplementedException();
        }

        public Task<PostingViewModel> GetEmptyPosting(long categoryId)
        {
            var postingViewModel = new PostingViewModel();

            postingViewModel.CategoryId = categoryId;
            postingViewModel.UserId = _currentUser.GetUserId<long>();
            postingViewModel.Title = string.Empty;
            postingViewModel.Description = string.Empty;
            postingViewModel.CreationDate = DateTime.Now;
            postingViewModel.ExpirationDate = DateTime.Now;
            postingViewModel.Type = (int) EPostingTypes.Buy;
            postingViewModel.Id = string.Empty;

            var postingCategory =
                _netBazaarDatabase.Categories.Include("CategoryFields")
                    .First(category => category.Id == postingViewModel.CategoryId);

            foreach (var categoryField in postingCategory.CategoryFields)
            {
                var postingFieldViewModel = new PostingFieldViewModel();
                postingFieldViewModel.Type = categoryField.Type;
                postingFieldViewModel.BooleanValue = false;
                postingFieldViewModel.DecimalValue = 0;
                postingFieldViewModel.IntegerValue = 0;
                postingFieldViewModel.TextValue = string.Empty;
                postingFieldViewModel.CategoryFieldId = categoryField.Id;
                postingFieldViewModel.CategoryFieldName =
                    categoryField
                        .Name;

                postingViewModel.Fields.Add(postingFieldViewModel);
            }

            return Task.FromResult(postingViewModel);
        }
    }
}