package via.group1.location_service.persistance.entity;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.experimental.SuperBuilder;
import via.group1.location_service.config.SqlConfig;

@Entity
@Getter
@Setter
@SuperBuilder
@AllArgsConstructor
@NoArgsConstructor
@Table(schema = SqlConfig.LOCATION_SCHEMA, name = "location")
public abstract class Location
{
  @Id
  @GeneratedValue
  private Long id;

  @OneToOne
  @JoinColumn(name = "address_id", referencedColumnName = "id")
  private Address address;

}
