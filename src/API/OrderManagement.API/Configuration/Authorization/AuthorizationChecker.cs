namespace OrderManagement.API.Configuration.Authorization
{
    public static class AuthorizationChecker
    {
        public static void CheckAllEndpoints()
        {
            var assembly = typeof(Assembly).Assembly;
            var allControllerTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ControllerBase)));

            List<string> notProtectedActionMethods = [];
            foreach (var controllerType in allControllerTypes)
            {
                var controllerHasPermissionAttribute = controllerType.GetCustomAttribute<HasPermissionAttribute>();
                if (controllerHasPermissionAttribute != null)
                {
                    continue;
                }

                var actionMethods = controllerType.GetMethods()
                    .Where(x => x.IsPublic && x.DeclaringType == controllerType)
                    .ToList();

                foreach (var publicMethod in actionMethods)
                {
                    var hasPermissionAttribute = publicMethod.GetCustomAttribute<HasPermissionAttribute>();
                    if (hasPermissionAttribute == null)
                    {
                        var noPermissionRequired = publicMethod.GetCustomAttribute<NoPermissionRequiredAttribute>();

                        if (noPermissionRequired == null)
                        {
                            notProtectedActionMethods.Add($"{controllerType.Name}.{publicMethod.Name}");
                        }
                    }
                }
            }

            if (notProtectedActionMethods.Any())
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Invalid authorization configuration: ");

                foreach (var notProtectedActionMethod in notProtectedActionMethods)
                {
                    errorBuilder.AppendLine($"Method {notProtectedActionMethod} is not protected. ");
                }

                throw new ApplicationException(errorBuilder.ToString());
            }
        }
    }
}