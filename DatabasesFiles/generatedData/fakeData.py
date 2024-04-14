from faker import Faker
import random
from datetime import datetime, timedelta

fake = Faker()

def generate_users_data(num_rows):
    users_data = []
    
    for _ in range(num_rows):
        first_name = fake.first_name()
        last_name = fake.last_name()
        # Generate a random date of birth between 1950 and 2005
        start_date = datetime(1950, 1, 1)
        end_date = datetime(2005, 12, 31)
        random_date = start_date + timedelta(days=random.randint(0, (end_date - start_date).days))
        date_of_birth = random_date.strftime('%Y-%m-%d')
        
        users_data.append({
            'first_name': first_name,
            'last_name': last_name,
            'date_of_birth': date_of_birth
        })
    
    return users_data

# Generate 100 rows of user data
users_data = generate_users_data(100)

# Output the generated data
for user in users_data:
    print(user)