namespace OrderManagement.API.Configuration.Extensions
{
	internal static class SwaggerExtensions
	{
		internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Order Management API",
					Version = "v1",
					Description = "Order Management API for modular monolith .NET application."
				});
				options.CustomSchemaIds(t => t.ToString());

				var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				var commentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var commentsFile = Path.Combine(baseDirectory, commentsFileName);
				options.IncludeXmlComments(commentsFile);

				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",					
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header
						},
						//Array.Empty<string>()
						new List<string>()
					}
				});
			});

			return services;
		}

		internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
		{
			//implement swagger documents for api
			app.UseSwagger(c =>
			{
				c.SerializeAsV2 = true;
			});
			
			app.UseSwaggerUI(c => 
			{
				c.RoutePrefix = "swagger";
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API");
				c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
				c.ShowExtensions();
			});

			return app;
		}
	}
}
