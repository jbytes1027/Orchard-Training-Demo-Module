using Lombiq.TrainingDemo.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.TrainingDemo.Settings;

// It's in the Settings folder by convention, but it's the same DisplayDriver as the others; except, it also has a
// specific base class. Don't forget to register this class with the service provider (see: Startup.cs).
public class ColorFieldSettingsDriver : ContentPartFieldDefinitionDisplayDriver<ColorField>
{
    // This won't have a Display override since it wouldn't make too much sense, settings are just edited.
    public override IDisplayResult Edit(ContentPartFieldDefinition model, BuildEditorContext context) =>
        // Same old Initialize shape helper.
        Initialize<ColorFieldSettings>(
                $"{nameof(ColorFieldSettings)}_Edit",
                settings => model.Settings.ToObject<ColorFieldSettings>().CopyTo(settings))
        .Location("Content");

    // ColorFieldSettings.Edit.cshtml file will contain the editor inputs.

    public override async Task<IDisplayResult> UpdateAsync(
        ContentPartFieldDefinition model,
        UpdatePartFieldEditorContext context)
    {
        var settings = await context.CreateModelAsync<ColorFieldSettings>(Prefix);

        // A content field or a content part can have multiple settings. These settings are stored in a single JSON
        // object. This helper will merge our settings into this JSON object so these will be stored.
        context.Builder.WithSettings(settings);

        return await EditAsync(model, context);
    }
}

// NEXT STATION: Views/ColorField.Edit.cshtml
