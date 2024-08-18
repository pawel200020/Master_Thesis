package org.dbFrameworks.entities;

import jakarta.persistence.*;

import java.util.List;

@Entity
public class Stores {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long StoreId;
    private String Address;
    private String Country;

    public List<org.dbFrameworks.entities.Orders> getOrders() {
        return Orders;
    }

    public void setOrders(List<org.dbFrameworks.entities.Orders> orders) {
        Orders = orders;
    }

    @OneToMany(cascade = CascadeType.REMOVE)
    @JoinColumn(name="StoreId")
    private List<Orders> Orders;

    public String getCountry() {
        return Country;
    }

    public void setCountry(String country) {
        Country = country;
    }

    public String getAddress() {
        return Address;
    }

    public void setAddress(String address) {
        Address = address;
    }

    public Long getStoreId() {
        return StoreId;
    }

    public void setStoreId(Long storeId) {
        StoreId = storeId;
    }
}
