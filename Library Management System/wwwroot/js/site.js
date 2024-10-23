// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
function toggleFilterMenu() {
    const filterMenu = document.getElementById('filterMenu');
    if (filterMenu.classList.contains('open')) {
        filterMenu.classList.remove('open');
        setTimeout(() => {
            filterMenu.style.display = 'none'; // Hide after transition
        }, 300); // Match the duration of the CSS transition
    } else {
        filterMenu.style.display = 'block'; // Show the drawer
        setTimeout(() => {
            filterMenu.classList.add('open'); // Slide in after display
        }, 10); // Small delay to ensure display is set
    }
}



function toggleSortOptions(event) {
    event.stopPropagation(); // Prevent closing the filter menu
    const options = document.getElementById('sortOptions');
    options.style.display = options.style.display === 'none' ? 'block' : 'none';
    hideOtherOptions('sortOptions');
}


function toggleGenreOptions(event) {
    event.stopPropagation();
    const options = document.getElementById('genreOptions');
    options.style.display = options.style.display === 'none' ? 'block' : 'none';
    hideOtherOptions('genreOptions');
}


function toggleAuthorOptions(event) {
    event.stopPropagation();
    const options = document.getElementById('authorOptions');
    options.style.display = options.style.display === 'none' ? 'block' : 'none';
    hideOtherOptions('authorOptions');
}


function hideOtherOptions(except) {
    const options = ['sortOptions', 'genreOptions', 'authorOptions'];
    options.forEach(option => {
        if (option !== except) {
            document.getElementById(option).style.display = 'none';
        }
    });
}


function sortBooks(order) {
    const books = Array.from(document.querySelectorAll('.book-item'));
    books.sort((a, b) => {
        const titleA = a.querySelector('h3').innerText.toLowerCase();
        const titleB = b.querySelector('h3').innerText.toLowerCase();
        if (order === 'asc') {
            return titleA.localeCompare(titleB);
        } else {
            return titleB.localeCompare(titleA);
        }
    });
    
    const container = document.getElementById('cardViewContainer');
    container.innerHTML = ''; // Clear the container
    books.forEach(book => container.appendChild(book)); // Re-append sorted books

        // Sort for list view
        const tableRows = Array.from(document.querySelectorAll('#listViewContainer tbody tr'));
        tableRows.sort((a, b) => {
            const titleA = a.cells[0].innerText.toLowerCase();
            const titleB = b.cells[0].innerText.toLowerCase();
            return order === 'asc' ? titleA.localeCompare(titleB) : titleB.localeCompare(titleA);
        });
    
        // Update list view container
        const listContainer = document.querySelector('#listViewContainer tbody');
        listContainer.innerHTML = ''; // Clear the table body
        tableRows.forEach(row => listContainer.appendChild(row)); // Re-append sorted rows
    
        // Optionally close the filter menu
        document.getElementById('filterMenu').style.display = 'none';
        document.getElementById('sortOptions').style.display = 'none';
}

let selectedGenre = '';
let selectedAuthor = '';

function applyGenre(genre) {
    const books = Array.from(document.querySelectorAll('.book-item'));
    books.forEach(book => {
        const bookGenre = book.querySelector('p:nth-child(3)').innerText.trim(); // Adjust if genre is in a different position
        book.style.display = (bookGenre === genre) ? 'block' : 'none';
    });
    selectedGenre = genre; // Store selected genre
    applyFilters();
    
    // Optionally close the filter menu after applying the filter
    document.getElementById('filterMenu').style.display = 'none';
    document.getElementById('genreOptions').style.display = 'none';
}
function applyAuthor(author) {
    const books = Array.from(document.querySelectorAll('.book-item'));
    books.forEach(book => {
        const bookAuthor = book.querySelector('p:nth-child(2)').innerText.trim(); // Adjust if author is in a different position
        book.style.display = (bookAuthor === author) ? 'block' : 'none';
    });
    selectedAuthor = author; // Store selected author
    applyFilters();
    // Optionally close the filter menu after applying the filter
    document.getElementById('filterMenu').style.display = 'none';
    document.getElementById('authorOptions').style.display = 'none';
}


function toggleView(viewType) {
    const cardContainer = document.getElementById('cardViewContainer');
    const listContainer = document.getElementById('listViewContainer');
    
    if (viewType === 'card') {
        cardContainer.style.display = 'flex';
        listContainer.style.display = 'none';
    } else {
        cardContainer.style.display = 'none';
        listContainer.style.display = 'table';
    }
}

function toggleUserMenu() {
    const menu = document.getElementById('userMenu');
    menu.style.display = menu.style.display === 'none' ? 'block' : 'none';
}

function addToCheckout(bookId) {
    // Implement the logic to add the book to the checkout page
    alert('Book ' + bookId + ' added to checkout!');
}
    
    
function searchBooks() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const bookItems = document.querySelectorAll('.book-item');
    const tableRows = document.querySelectorAll('#listViewContainer tbody tr');

    // Card View Search
    bookItems.forEach(item => {
        const title = item.querySelector('h3').innerText.toLowerCase();
        const author = item.querySelector('p:nth-child(2)').innerText.toLowerCase();
        const genre = item.querySelector('p:nth-child(3)').innerText.toLowerCase();

        if (title.includes(searchTerm) || author.includes(searchTerm) || genre.includes(searchTerm)) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });

    // List View Search
    tableRows.forEach(row => {
        const title = row.cells[0].innerText.toLowerCase();
        const author = row.cells[1].innerText.toLowerCase();
        const genre = row.cells[2].innerText.toLowerCase();

        if (title.includes(searchTerm) || author.includes(searchTerm) || genre.includes(searchTerm)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
    }



function applyFilters() {
    const selectedGenre = document.getElementById('genreFilter').value;
    const sortOrder = document.getElementById('sortOrder').value;
    const authorName = document.getElementById('authorFilter').value.toLowerCase();
    
    const bookItems = document.querySelectorAll('.book-item');
    const tableRows = document.querySelectorAll('#listViewContainer tbody tr');

    // Card View Filtering
    let filteredBooks = Array.from(bookItems).filter(item => {
        const genre = item.querySelector('p:nth-child(3)').innerText; // Genre from card view
        const author = item.querySelector('p:nth-child(2)').innerText.toLowerCase(); // Author from card view
        
        const matchesGenre = selectedGenre === "" || genre === selectedGenre;
        const matchesAuthor = author.includes(authorName.toLowerCase());
        
        return matchesGenre && matchesAuthor;
    });

    // Sorting Card View
    if (sortOrder) {
        filteredBooks.sort((a, b) => {
            const titleA = a.querySelector('h3').innerText.toLowerCase();
            const titleB = b.querySelector('h3').innerText.toLowerCase();
            return sortOrder === 'asc' ? titleA.localeCompare(titleB) : titleB.localeCompare(titleA);
        });
    }

    // Display filtered and sorted books
    bookItems.forEach(item => item.style.display = 'none'); // Hide all initially
    filteredBooks.forEach(item => item.style.display = 'block'); // Show filtered and sorted

    // List View Filtering
    let filteredRows = Array.from(tableRows).filter(row => {
        const genre = row.cells[2].innerText; // Genre from list view
        const author = row.cells[1].innerText.toLowerCase(); // Author from list view

        const matchesGenre = selectedGenre === "" || genre === selectedGenre;
        const matchesAuthor = author.includes(authorName.toLowerCase());
        
        return matchesGenre && matchesAuthor;
    });

    // Sorting List View
    if (sortOrder) {
        filteredRows.sort((a, b) => {
            const titleA = a.cells[0].innerText.toLowerCase();
            const titleB = b.cells[0].innerText.toLowerCase();
            return sortOrder === 'asc' ? titleA.localeCompare(titleB) : titleB.localeCompare(titleA);
        });
    }

    // Display filtered and sorted rows
    tableRows.forEach(row => row.style.display = 'none'); // Hide all initially
    filteredRows.forEach(row => row.style.display = ''); // Show filtered and sorted
}


function removeRow(button) {
    const row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);
}

function submitCheckout() {
    const rows = document.querySelectorAll('#booksTable tbody tr');
    const dueDates = [];
    
    rows.forEach(row => {
        const title = row.cells[0].innerText;
        const dueDate = row.querySelector('.due-date').value;
        if (dueDate) {
            dueDates.push({ title, dueDate });
        }
    });

    document.getElementById('checkoutButton').addEventListener('click', function() {
        const dueDate = document.getElementById('dueDateInput').value; // Get the due date input
        const borrowedBooks = []; // Assuming you have a way to gather selected books
    
        // Gather borrowed books data
        document.querySelectorAll('.book-row:checked').forEach(row => {
            borrowedBooks.push({
                bookId: row.dataset.bookId,
                dueDate: dueDate
            });
        });
    
        if (borrowedBooks.length === 0) {
            alert('Please select at least one book to borrow.');
            return;
        }
    
        // Send data to the checkout action in AccountController
        $.ajax({
            type: 'POST',
            url: '/Account/SubmitCheckout', // Adjust the URL to your controller action
            contentType: 'application/json',
            data: JSON.stringify(borrowedBooks),
            success: function(data) {
                document.getElementById('confirmationMessage').innerText = 'Books reserved successfully!';
                document.getElementById('booksTable').style.display = 'none'; // Hide the table
            },
            error: function(error) {
                console.error('Error:', error);
            }
        });
    });
    
}
const drawer = document.getElementById('drawer');
const toggleBtn = document.getElementById('toggleBtn');


// Toggle the drawer
toggleBtn.onclick = function() {
    if (drawer.classList.contains('open')) {
        drawer.classList.remove('open');
        setTimeout(() => {
            drawer.style.display = 'none'; // Hide after transition
        }, 300); // Match the duration of the CSS transition
    } else {
        drawer.style.display = 'block'; // Show the drawer
        setTimeout(() => {
            drawer.classList.add('open'); // Slide in after display
        }, 10); // Small delay to ensure display is set
    }
}