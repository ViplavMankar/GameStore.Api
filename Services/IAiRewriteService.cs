using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IAiRewriteService
{
    Task<RewriteResult> RewriteAsync(RewriteRequest req, CancellationToken ct);
}
