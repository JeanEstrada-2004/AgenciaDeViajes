using AgenciaDeViajes.Models;
using AgenciaDeViajes.Data;
using Newtonsoft.Json;

namespace AgenciaDeViajes.Services
{
    public static class SeedData
    {
        public static async Task CargarFestividadesDesdeJsonAsync(ApplicationDbContext context, string jsonPath)
        {
            if (!context.Festividades.Any())
            {
                if (File.Exists(jsonPath))
                {
                    string json = await File.ReadAllTextAsync(jsonPath);
                    var festividades = JsonConvert.DeserializeObject<List<Festividad>>(json);

                    if (festividades != null)
                    {
                        foreach (var festividad in festividades)
                        {
                            festividad.Fecha = DateTime.SpecifyKind(festividad.Fecha, DateTimeKind.Unspecified);
                            context.Festividades.Add(festividad);
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
