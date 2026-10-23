# BBC Website Functional Test Cases

## TC001 - Verify BBC Home Page Loads Successfully

**Test Case ID:** TC001  
**Test Scenario:** Home Page Accessibility  
**Priority:** High

### Preconditions
- Internet connection is available.
- Supported browser is installed.

### Test Steps
1. Open a web browser.
2. Navigate to https://www.bbc.com.

### Expected Result
- BBC homepage loads successfully.
- BBC logo is displayed.
- Main navigation menu is visible.
- No error messages are displayed.

---

## TC002 - Verify Search Functionality

**Test Case ID:** TC002  
**Test Scenario:** Search News Articles  
**Priority:** High

### Preconditions
- User is on the BBC homepage.

### Test Steps
1. Click the Search icon.
2. Enter the keyword **Technology**.
3. Click Search or press Enter.

### Expected Result
- Search results page is displayed.
- Relevant articles related to "Technology" are shown.
- User can open an article from the search results.

---

## TC003 - Verify Navigation to News Section

**Test Case ID:** TC003  
**Test Scenario:** News Menu Navigation  
**Priority:** High

### Preconditions
- User is on the BBC homepage.

### Test Steps
1. Click the **News** menu option from the navigation bar.

### Expected Result
- News section opens successfully.
- Headlines and news content are displayed.
- URL is updated to the News section.

---

## TC004 - Verify Opening a News Article

**Test Case ID:** TC004  
**Test Scenario:** News Article Access  
**Priority:** Medium

### Preconditions
- User is on the BBC homepage or News page.

### Test Steps
1. Click any available news headline.
2. Wait for the article page to load.

### Expected Result
- Selected article opens successfully.
- Article title is displayed.
- Article content is visible.
- Images and videos (if available) load correctly.

---

## TC005 - Verify Responsive Layout in Mobile View

**Test Case ID:** TC005  
**Test Scenario:** Mobile Responsiveness  
**Priority:** Medium

### Preconditions
- Browser developer tools or mobile device is available.

### Test Steps
1. Open https://www.bbc.com.
2. Switch to mobile view (e.g., iPhone/Android resolution).
3. Navigate through Home, News, and Search sections.

### Expected Result
- Website layout adjusts to mobile screen size.
- Navigation menu is accessible.
- Content is readable.
- Links and buttons function correctly.
- No horizontal scrolling is required.

---

# Test Summary

| Test Case ID | Test Scenario | Priority |
|--------------|--------------|----------|
| TC001 | Homepage Load Verification | High |
| TC002 | Search Functionality | High |
| TC003 | News Section Navigation | High |
| TC004 | Open News Article | Medium |
| TC005 | Mobile Responsiveness | Medium |