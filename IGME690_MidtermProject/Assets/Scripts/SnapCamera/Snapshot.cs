using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SnapshotSubject
{
    public string SubjectType { get; set; }
    public float SubjectDistance { get; set; }
    public string SubjectAction { get; set; }
    public bool IsSubjectObscured { get; set; }

    public override string ToString()
    {
        return $"Type: {SubjectType}\n" +
            $"Distance: {SubjectDistance}\n" +
            $"Action: {SubjectAction}\n" +
            $"Subject is obscured: {IsSubjectObscured}";
    }
}

/// <summary>
/// Represents a snapshot. This is more of a data-storage class
/// than a proper Unity GameObject class, but if it needs to be
/// a GameObject later on, then that can be changed
/// </summary>
public class Snapshot
{
    // The subjects that are in the picture.
    private List<SnapshotSubject> subjects;
    // The weather that was occuring when the snapshot was taken.
   // private WeatherController.WeatherState weatherState;
    // The time of day when the snapshot was taken.
    private DateTime snapshotTime;
    // A reference to the snapshot picture itself.
    private Texture2D picture;
    // A reference to the file path of the snapshot.
    private string filePath;

    /// <summary>
    /// The list of subjects in the photo. Get-only.
    /// </summary>
    public List<SnapshotSubject> Subjects
    {
        get { return subjects; }
    }

    /// <summary>
    /// The state of the weather of the photo. Get-only.
    /// </summary>
/*    public WeatherController.WeatherState WeatherState
    {
        get { return weatherState; }
    }*/

    /// <summary>
    /// The time that the snapshot was taken at. Get-only.
    /// </summary>
    public DateTime SnapshotTime
    {
        get { return snapshotTime; }
    }

    /// <summary>
    /// The Texture2D used for the picture. Get-only.
    /// </summary>
    public Texture2D Picture
    {
        get { return picture; }
    }

    /// <summary>
    /// The file path of the picture. Get-only.
    /// </summary>
    public string FilePath
    {
        get { return filePath; }
    }

    public Snapshot(DateTime snapshotTime, Texture2D picture, string filePath)
    {
/*        this.subjects = subjects;
        this.weatherState = weatherState;*/
        this.snapshotTime = snapshotTime;
        this.picture = picture;
        this.filePath = filePath;
    }

    public override string ToString()
    {
        string str = "Subjects: ";
        foreach (SnapshotSubject subject in subjects)
        {
            str += subject + " ";
        }
        str += $"Snapshot time: {snapshotTime}\n" +
            $"File path: {filePath}";

        return str;
    }
}
