package org.dbFrameworks.entities;

import jakarta.persistence.*;

import java.util.List;

@Entity
public class Employees {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int EmployeeId;
    private String FirstName;
    private String LastName;
    private String PhoneNumber;
    private String Adress;
    @ManyToOne
    @JoinColumn(name="PositionId")
    private Positions Position;
    @OneToMany
    @JoinColumn(name = "EmployeeId")
    private List<Orders> Orders;

    public Positions getPosition() {
        return Position;
    }

    public void setPosition(Positions position) {
        Position = position;
    }

    public int getEmployeeId() {
        return EmployeeId;
    }

    public void setEmployeeId(int EmployeeId) {
        this.EmployeeId = EmployeeId;
    }

    public String getFirstName() {
        return FirstName;
    }

    public void setFirstName(String firstName) {
        FirstName = firstName;
    }

    public String getLastName() {
        return LastName;
    }

    public void setLastName(String lastName) {
        LastName = lastName;
    }

    public String getPhoneNumber() {
        return PhoneNumber;
    }

    public void setPhoneNumber(String phoneNumber) {
        PhoneNumber = phoneNumber;
    }

    public String getAdress() {
        return Adress;
    }

    public void setAdress(String address) {
        Adress = address;
    }

    public List<org.dbFrameworks.entities.Orders> getOrders() {
        return Orders;
    }

    public void setOrders(List<org.dbFrameworks.entities.Orders> orders) {
        Orders = orders;
    }

}
