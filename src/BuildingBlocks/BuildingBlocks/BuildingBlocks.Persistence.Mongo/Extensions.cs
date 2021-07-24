using System;
using BuildingBlocks.Persistence.Mongo.Factories;
using BuildingBlocks.Persistence.Mongo.Repositories;
using BuildingBlocks.Persistence.Mongo.Seeders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public static class Extensions
    {
        private const string SectionName = "mongo";
        private static bool _conventionsRegistered;

        public static IServiceCollection AddMongoRepository<TEntity, TIdentifiable>(this IServiceCollection services,
            string collectionName)
            where TEntity : IIdentifiable<TIdentifiable>
        {
            services.AddTransient<IMongoRepository<TEntity, TIdentifiable>>(sp =>
            {
                var database = sp.GetService<IMongoDatabase>();
                return new MongoRepository<TEntity, TIdentifiable>(database, collectionName);
            });

            return services;
        }

        public static IServiceCollection AddMongoPersistence(this IServiceCollection services,
            IConfiguration configuration, string sectionName = SectionName, Type seederType = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var options = configuration.GetSection(sectionName).Get<MongoOptions>();
            services.AddOptions<MongoOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();

            services.AddScoped<IMongoDbContext, MongoDbContext>();
            services.AddSingleton<IMongoClient>(sp => new MongoClient(options?.ConnectionString));
            services.AddTransient(sp =>
            {
                var client = sp.GetService<IMongoClient>();
                return client?.GetDatabase(options?.Database);
            });
            services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();

            if (seederType is null)
                services.AddTransient<IMongoDbSeeder, MongoDbSeeder>();
            else
                services.AddTransient(typeof(IMongoDbSeeder), seederType);

            if (!_conventionsRegistered) RegisterConventions();

            return services;
        }

        private static void RegisterConventions()
        {
            _conventionsRegistered = true;
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            ConventionRegistry.Register("shopping", new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)
            }, _ => true);
        }
    }
}