package via.group1.location_service.model.interfaces;

import via.group1.location_service.persistance.entity.Location;
import via.group1.location_service.persistance.entity.PickUpPoint;

import java.util.ArrayList;

public interface LocationService
{
  Location saveLocation(Location location);
  Location getLocation(Long Id);
  ArrayList<Location> getAllLocations();
}
