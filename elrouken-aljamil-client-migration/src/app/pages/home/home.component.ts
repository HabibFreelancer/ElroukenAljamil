import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [RouterLink, NgFor],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  categories = [
    {
      name: "Voitures d'occasion",
      ads: [
        { title: 'Peugeot 308 2019', price: '12 500 €', location: 'Paris 75001', image: 'https://images.unsplash.com/photo-1549924231-f129b911e442?w=300&h=375&fit=crop' },
        { title: 'Renault Clio V', price: '9 800 €', location: 'Lyon 69001', image: 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=300&h=375&fit=crop' },
        { title: 'BMW Série 3 2020', price: '28 000 €', location: 'Marseille 13001', image: 'https://images.unsplash.com/photo-1555215695-3004980ad54e?w=300&h=375&fit=crop' },
        { title: 'Volkswagen Golf 8', price: '22 500 €', location: 'Toulouse 31000', image: 'https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=300&h=375&fit=crop' },
        { title: 'Audi A3 Sportback', price: '19 900 €', location: 'Bordeaux 33000', image: 'https://images.unsplash.com/photo-1542362567-b07e54358753?w=300&h=375&fit=crop' },
        { title: 'Mercedes Classe A', price: '24 000 €', location: 'Nantes 44000', image: 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=300&h=375&fit=crop' },
        { title: 'Citroën C3 2021', price: '11 200 €', location: 'Lille 59000', image: 'https://images.unsplash.com/photo-1494976388531-d1058494cdd8?w=300&h=375&fit=crop' },
        { title: 'Ford Focus ST', price: '15 800 €', location: 'Strasbourg 67000', image: 'https://images.unsplash.com/photo-1583121274602-3e2820c69888?w=300&h=375&fit=crop' },
        { title: 'Toyota Yaris Hybride', price: '14 500 €', location: 'Nice 06000', image: 'https://images.unsplash.com/photo-1580273916550-e323be2ae537?w=300&h=375&fit=crop' },
        { title: 'Fiat 500 électrique', price: '18 900 €', location: 'Montpellier 34000', image: 'https://images.unsplash.com/photo-1609521263047-f8f205293f24?w=300&h=375&fit=crop' },
      ]
    },
    {
      name: 'Locations appartements',
      ads: [
        { title: 'Studio 25m² centre-ville', price: '550 €/mois', location: 'Paris 75011', image: 'https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=300&h=375&fit=crop' },
        { title: 'T2 lumineux balcon', price: '750 €/mois', location: 'Lyon 69003', image: 'https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=300&h=375&fit=crop' },
        { title: 'T3 rénové parking', price: '900 €/mois', location: 'Bordeaux 33000', image: 'https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=300&h=375&fit=crop' },
        { title: 'Loft 60m² atypique', price: '1 200 €/mois', location: 'Marseille 13006', image: 'https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=300&h=375&fit=crop' },
        { title: 'T2 meublé terrasse', price: '680 €/mois', location: 'Toulouse 31000', image: 'https://images.unsplash.com/photo-1484154218962-a197022b5858?w=300&h=375&fit=crop' },
        { title: 'Studio étudiant', price: '420 €/mois', location: 'Rennes 35000', image: 'https://images.unsplash.com/photo-1536376072261-38c75010e6c9?w=300&h=375&fit=crop' },
        { title: 'T4 familial jardin', price: '1 100 €/mois', location: 'Nantes 44000', image: 'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=300&h=375&fit=crop' },
        { title: 'T1 bis proche métro', price: '600 €/mois', location: 'Lille 59000', image: 'https://images.unsplash.com/photo-1554995207-c18c203602cb?w=300&h=375&fit=crop' },
        { title: 'Duplex 80m² charme', price: '1 350 €/mois', location: 'Strasbourg 67000', image: 'https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=300&h=375&fit=crop' },
        { title: 'T2 vue mer', price: '850 €/mois', location: 'Nice 06000', image: 'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=300&h=375&fit=crop' },
      ]
    },
    {
      name: 'Smartphones reconditionnés',
      ads: [
        { title: 'iPhone 14 Pro 128Go', price: '650 €', location: 'Paris 75008', image: 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=300&h=375&fit=crop' },
        { title: 'Samsung Galaxy S23', price: '480 €', location: 'Lyon 69002', image: 'https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=300&h=375&fit=crop' },
        { title: 'iPhone 13 256Go', price: '520 €', location: 'Marseille 13001', image: 'https://images.unsplash.com/photo-1632633173522-47456de71b76?w=300&h=375&fit=crop' },
        { title: 'Google Pixel 7', price: '380 €', location: 'Toulouse 31000', image: 'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=300&h=375&fit=crop' },
        { title: 'OnePlus 11 5G', price: '420 €', location: 'Bordeaux 33000', image: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=375&fit=crop' },
        { title: 'iPhone 12 64Go', price: '350 €', location: 'Nantes 44000', image: 'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=300&h=375&fit=crop' },
        { title: 'Samsung Galaxy A54', price: '250 €', location: 'Lille 59000', image: 'https://images.unsplash.com/photo-1585060544812-6b45742d762f?w=300&h=375&fit=crop' },
        { title: 'Huawei P60 Pro', price: '450 €', location: 'Strasbourg 67000', image: 'https://images.unsplash.com/photo-1574944985070-8f3ebc6b79d2?w=300&h=375&fit=crop' },
        { title: 'iPhone SE 2022', price: '280 €', location: 'Nice 06000', image: 'https://images.unsplash.com/photo-1556656793-08538906a9f8?w=300&h=375&fit=crop' },
        { title: 'Sony Xperia 5 IV', price: '390 €', location: 'Montpellier 34000', image: 'https://images.unsplash.com/photo-1512054502232-10a0a035d672?w=300&h=375&fit=crop' },
      ]
    },
    {
      name: 'Meubles vintage',
      ads: [
        { title: 'Table basse scandinave', price: '120 €', location: 'Paris 75012', image: 'https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=300&h=375&fit=crop' },
        { title: 'Buffet années 60', price: '280 €', location: 'Lyon 69007', image: 'https://images.unsplash.com/photo-1506439773649-6e0eb8cfb237?w=300&h=375&fit=crop' },
        { title: 'Fauteuil club cuir', price: '350 €', location: 'Bordeaux 33000', image: 'https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?w=300&h=375&fit=crop' },
        { title: 'Commode art déco', price: '200 €', location: 'Marseille 13008', image: 'https://images.unsplash.com/photo-1595428774223-ef52624120d2?w=300&h=375&fit=crop' },
        { title: 'Étagère industrielle', price: '95 €', location: 'Toulouse 31000', image: 'https://images.unsplash.com/photo-1532372320572-cda25653a26d?w=300&h=375&fit=crop' },
        { title: 'Chaises bistrot x4', price: '160 €', location: 'Nantes 44000', image: 'https://images.unsplash.com/photo-1503602642458-232111445657?w=300&h=375&fit=crop' },
        { title: 'Bureau en chêne massif', price: '450 €', location: 'Lille 59000', image: 'https://images.unsplash.com/photo-1518455027359-f3f8164ba6bd?w=300&h=375&fit=crop' },
        { title: 'Miroir doré ancien', price: '75 €', location: 'Strasbourg 67000', image: 'https://images.unsplash.com/photo-1618220179428-22790b461013?w=300&h=375&fit=crop' },
        { title: 'Canapé velours vert', price: '580 €', location: 'Nice 06000', image: 'https://images.unsplash.com/photo-1550581190-9c1c48d21d6c?w=300&h=375&fit=crop' },
        { title: 'Lampadaire laiton', price: '110 €', location: 'Montpellier 34000', image: 'https://images.unsplash.com/photo-1507473885765-e6ed057ab6fe?w=300&h=375&fit=crop' },
      ]
    },
    {
      name: 'Vélos électriques',
      ads: [
        { title: 'VTT électrique Decathlon', price: '900 €', location: 'Paris 75015', image: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=300&h=375&fit=crop' },
        { title: 'Vélo ville électrique', price: '750 €', location: 'Lyon 69006', image: 'https://images.unsplash.com/photo-1558618666-fcd25c85f82e?w=300&h=375&fit=crop' },
        { title: 'E-bike pliant compact', price: '650 €', location: 'Bordeaux 33000', image: 'https://images.unsplash.com/photo-1532298229144-0ec0c57515c7?w=300&h=375&fit=crop' },
        { title: 'VTC électrique Moustache', price: '1 200 €', location: 'Marseille 13001', image: 'https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=300&h=375&fit=crop' },
        { title: 'Vélo cargo électrique', price: '1 800 €', location: 'Toulouse 31000', image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=375&fit=crop' },
        { title: 'Speed bike 45km/h', price: '2 200 €', location: 'Nantes 44000', image: 'https://images.unsplash.com/photo-1576435728678-68d0fbf94e91?w=300&h=375&fit=crop' },
        { title: 'E-VTT tout suspendu', price: '1 500 €', location: 'Lille 59000', image: 'https://images.unsplash.com/photo-1596738901637-5c1c6b3f1f1e?w=300&h=375&fit=crop' },
        { title: 'Vélo route électrique', price: '1 100 €', location: 'Strasbourg 67000', image: 'https://images.unsplash.com/photo-1517649763962-0c623066013b?w=300&h=375&fit=crop' },
        { title: 'Mini vélo électrique', price: '500 €', location: 'Nice 06000', image: 'https://images.unsplash.com/photo-1505705694340-019e0d529860?w=300&h=375&fit=crop' },
        { title: 'Fatbike électrique', price: '1 350 €', location: 'Montpellier 34000', image: 'https://images.unsplash.com/photo-1541625602330-2277a4c46182?w=300&h=375&fit=crop' },
      ]
    }
  ];
}
