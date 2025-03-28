﻿namespace Api.Application.Abstractions.Messaging;

public interface IMessageConsumer
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}