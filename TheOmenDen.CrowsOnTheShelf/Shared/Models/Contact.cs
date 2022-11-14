using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;
public sealed record Contact([property: Required(AllowEmptyStrings =false)] [property: JsonPropertyName("name")] string Name, [property: Required(AllowEmptyStrings = false)] [property: JsonPropertyName("email")] string Email , [property: Required(AllowEmptyStrings = false)] [property: JsonPropertyName("message")] string Message);
