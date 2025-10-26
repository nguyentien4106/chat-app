﻿using MediatR;

namespace ChatApp.Application.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
}