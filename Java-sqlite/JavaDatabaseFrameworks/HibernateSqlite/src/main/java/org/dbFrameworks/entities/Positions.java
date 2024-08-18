package org.dbFrameworks.entities;

import jakarta.persistence.*;

import java.util.List;

@Entity
public class Positions {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private long PositionId;
    private String PositionName;
    private String CarParkingPlace;
    private String PositionBonus;
    @OneToMany
    @JoinColumn(name="PositionId")
    private List<Employees> Employee;

    public List<Employees> getEmployee() {
        return Employee;
    }

    public void setEmployee(List<Employees> employee) {
        Employee = employee;
    }

    public long getPositionId() {
        return PositionId;
    }

    public void setPositionId(long positionId) {
        PositionId = positionId;
    }

    public String getPositionName() {
        return PositionName;
    }

    public void setPositionName(String positionName) {
        PositionName = positionName;
    }

    public String getCarParkingPlace() {
        return CarParkingPlace;
    }

    public void setCarParkingPlace(String carParkingPlace) {
        CarParkingPlace = carParkingPlace;
    }

    public String getPositionBonus() {
        return PositionBonus;
    }

    public void setPositionBonus(String positionBonus) {
        PositionBonus = positionBonus;
    }
}
