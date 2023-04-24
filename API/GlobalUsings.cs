global using Serilog;
global using Sentry;

global using MediatR;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.OutputCaching;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Diagnostics;

global using Broker.Common;
global using Broker.Domain;
global using Broker.Infrastructure;
global using Broker.Infrastructure.Interfaces;
global using Broker.Application;
global using Broker.Application.DataTransferObjects;
global using Broker.Application.Validators;
global using Broker.Application.Interfaces;
global using Broker.Application.Settings;
global using Broker.API.Controllers;
global using Broker.API.Settings;