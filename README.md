# License Manager for Unity
A simple tool to find all LICENSE files in Unity project and get a report on all these files.
This is might be very helpful if you need to reveal info about all third-party assets used in your project.
Works with Assets and Packages folders.

#### How to use?
1. Create `LicenseCollector` asset via `Create -> License Manager -> Collector` context menu somethere in project
2. Press button **Refresh**
3. To get runtime report use this code:
```cs
[SerializeField]
private LicenseManager.LicenseCollector _collector;

void Start()
{
    var report = _collector.GetReport();
    foreach (var entry in report)
    {
        //do something here with each entry
        string name = entry.name;
        string content = entry.content;
        string license = entry.license;
        LicenseManager.LicenseRemarks remarks = entry.remarks;

        if (remarks.HasFlag(LicenseManager.LicenseRemarks.NonCommercial))
        {
            Debug.LogWarning($"{name} asset could be used but only in non-commercial projects, because it has {license}");
        }
        if (remarks.HasFlag(LicenseManager.LicenseRemarks.Viral))
        {
            Debug.LogError($"{name} asset has viral {license} and probably cannot be placed in this project");
        }
    }
}
```

#### Unity Plugin
The latest version can be installed via [package manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using following git URL: \
`https://github.com/am1goo/unity-plugin-license-manager.git#0.0.5`

#### What next?
- [ ] Build post-process callback (to get correct report up-to-date)

## Tested in
- **Unity 2020.3.x**
- **Unity 2022.3.x**

## Contribute
Contribution in any form is very welcome. Bugs, feature requests or feedback can be reported in form of Issues.
