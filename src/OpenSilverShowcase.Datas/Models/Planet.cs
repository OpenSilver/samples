using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace OpenSilverShowcase.Datas.Models;

public enum PlanetType
{
    [Description("Terrestrial Planet")]
    Terrestrial,

    [Description("Gas Giant")]
    GasGiant,

    [Description("Ice Giant")]
    IceGiant,

    [Description("Dwarf Planet")]
    DwarfPlanet
}

public class Planet : INotifyPropertyChanged
{
    private string _name;
    private double _distanceFromSun;
    private double _diameter;
    private double _mass;
    private int _numberOfMoons;
    private bool _hasRings;
    private PlanetType _type;
    private DateTime? _discoveryDate;
    private string _description;

    [Required(ErrorMessage = "Planet name is required.")]
    [StringLength(50, ErrorMessage = "Planet name cannot exceed 50 characters.")]
    [Display(Name = "Planet Name")]
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    [Range(0.1, 10000, ErrorMessage = "Distance must be between 0.1 and 10000 AU.")]
    [Display(Name = "Distance from Sun (AU)")]
    public double DistanceFromSun
    {
        get => _distanceFromSun;
        set
        {
            if (_distanceFromSun != value)
            {
                _distanceFromSun = value;
                OnPropertyChanged(nameof(DistanceFromSun));
            }
        }
    }

    [Range(100, 200000, ErrorMessage = "Diameter must be between 100 and 200000 km.")]
    [Display(Name = "Diameter (km)")]
    public double Diameter
    {
        get => _diameter;
        set
        {
            if (_diameter != value)
            {
                _diameter = value;
                OnPropertyChanged(nameof(Diameter));
            }
        }
    }

    [Range(0.001, 1000, ErrorMessage = "Mass must be between 0.001 and 1000 Earth masses.")]
    [Display(Name = "Mass (Earth = 1)")]
    public double Mass
    {
        get => _mass;
        set
        {
            if (_mass != value)
            {
                _mass = value;
                OnPropertyChanged(nameof(Mass));
            }
        }
    }

    [Range(0, 1000, ErrorMessage = "Number of moons must be between 0 and 1000.")]
    [Display(Name = "Number of Moons")]
    public int NumberOfMoons
    {
        get => _numberOfMoons;
        set
        {
            if (_numberOfMoons != value)
            {
                _numberOfMoons = value;
                OnPropertyChanged(nameof(NumberOfMoons));
            }
        }
    }

    [Display(Name = "Has Rings")]
    public bool HasRings
    {
        get => _hasRings;
        set
        {
            if (_hasRings != value)
            {
                _hasRings = value;
                OnPropertyChanged(nameof(HasRings));
            }
        }
    }

    [Display(Name = "Planet Type")]
    public PlanetType Type
    {
        get => _type;
        set
        {
            if (_type != value)
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
    }

    [Display(Name = "Discovery Date")]
    public DateTime? DiscoveryDate
    {
        get => _discoveryDate;
        set
        {
            if (_discoveryDate != value)
            {
                _discoveryDate = value;
                OnPropertyChanged(nameof(DiscoveryDate));
            }
        }
    }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static ObservableCollection<Planet> GetListOfPlanets()
    {
        return new ObservableCollection<Planet>
        {
            new Planet
            {
                Name = "Mercury",
                DistanceFromSun = 0.39,
                Diameter = 4879,
                Mass = 0.055,
                NumberOfMoons = 0,
                HasRings = false,
                Type = PlanetType.Terrestrial,
                DiscoveryDate = null, // Known since ancient times
                Description = "The smallest planet in the solar system and the closest to the Sun."
            },
            new Planet
            {
                Name = "Venus",
                DistanceFromSun = 0.72,
                Diameter = 12104,
                Mass = 0.815,
                NumberOfMoons = 0,
                HasRings = false,
                Type = PlanetType.Terrestrial,
                DiscoveryDate = null,
                Description = "The hottest planet in the solar system with a thick atmosphere."
            },
            new Planet
            {
                Name = "Earth",
                DistanceFromSun = 1.0,
                Diameter = 12756,
                Mass = 1.0,
                NumberOfMoons = 1,
                HasRings = false,
                Type = PlanetType.Terrestrial,
                DiscoveryDate = null,
                Description = "The only planet known to support life."
            },
            new Planet
            {
                Name = "Mars",
                DistanceFromSun = 1.52,
                Diameter = 6792,
                Mass = 0.107,
                NumberOfMoons = 2,
                HasRings = false,
                Type = PlanetType.Terrestrial,
                DiscoveryDate = null,
                Description = "Known as the Red Planet, it has polar ice caps."
            },
            new Planet
            {
                Name = "Jupiter",
                DistanceFromSun = 5.20,
                Diameter = 142984,
                Mass = 317.8,
                NumberOfMoons = 95,
                HasRings = true,
                Type = PlanetType.GasGiant,
                DiscoveryDate = null,
                Description = "The largest planet with a giant storm known as the Great Red Spot."
            },
            new Planet
            {
                Name = "Saturn",
                DistanceFromSun = 9.58,
                Diameter = 120536,
                Mass = 95.2,
                NumberOfMoons = 146,
                HasRings = true,
                Type = PlanetType.GasGiant,
                DiscoveryDate = null,
                Description = "Famous for its beautiful rings."
            },
            new Planet
            {
                Name = "Uranus",
                DistanceFromSun = 19.20,
                Diameter = 51118,
                Mass = 14.5,
                NumberOfMoons = 27,
                HasRings = true,
                Type = PlanetType.IceGiant,
                DiscoveryDate = new DateTime(1781, 3, 13),
                Description = "A unique planet that rotates on its side."
            },
            new Planet
            {
                Name = "Neptune",
                DistanceFromSun = 30.05,
                Diameter = 49528,
                Mass = 17.1,
                NumberOfMoons = 16,
                HasRings = true,
                Type = PlanetType.IceGiant,
                DiscoveryDate = new DateTime(1846, 9, 23),
                Description = "The farthest planet from the Sun with extremely strong winds."
            },
            new Planet
            {
                Name = "Pluto",
                DistanceFromSun = 39.48,
                Diameter = 2376,
                Mass = 0.0022,
                NumberOfMoons = 5,
                HasRings = false,
                Type = PlanetType.DwarfPlanet,
                DiscoveryDate = new DateTime(1930, 2, 18),
                Description = "Reclassified as a dwarf planet in 2006."
            }
        };
    }

    public override string ToString()
    {
        return Name ?? "Unknown Planet";
    }
}
