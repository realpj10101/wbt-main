global using api.Models;
global using api.Settings;
global using Microsoft.AspNetCore.Mvc;
global using MongoDB.Bson;
global using MongoDB.Driver;
global using api.DTOs;
global using MongoDB.Driver.Linq; // needed for AnyAsync()
global using System.ComponentModel.DataAnnotations;
global using MongoDB.Bson.Serialization.Attributes;
global using api.Controllers.Helpers;
global using api.Interfaces;
global using api.Repositories;
global using Microsoft.Extensions.Options;
global using System.Text;
global using api.Services;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;