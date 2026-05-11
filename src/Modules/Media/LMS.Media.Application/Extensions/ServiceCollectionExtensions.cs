using LMS.Common.CQRS;
using LMS.Media.Application.Commands.ArchiveMedia;
using LMS.Media.Application.Commands.UploadMedia;
using LMS.Media.Application.Models;
using LMS.Media.Application.Queries.GetMedia;
using LMS.Media.Application.Queries.GetMediaByEntity;
using LMS.Media.Application.Queries.GetMediaUrl;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Media.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<UploadMediaCommand, UploadMediaResult>, UploadMediaCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveMediaCommand, ArchiveMediaResult>, ArchiveMediaCommandHandler>();

        services.AddScoped<IQueryHandler<GetMediaQuery, MediaFile?>, GetMediaQueryHandler>();
        services.AddScoped<IQueryHandler<GetMediaByEntityQuery, IReadOnlyList<MediaFile>>, GetMediaByEntityQueryHandler>();
        services.AddScoped<IQueryHandler<GetMediaUrlQuery, MediaReadUrl?>, GetMediaUrlQueryHandler>();

        return services;
    }
}
